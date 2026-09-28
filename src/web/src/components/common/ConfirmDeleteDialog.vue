<script setup lang="ts">
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from '@/components/ui/alert-dialog'

const props = defineProps<{
  open: boolean
  name: string
  loading?: boolean
}>()

const emit = defineEmits<{
  'update:open': [boolean]
  confirm: []
}>()
</script>

<template>
  <AlertDialog :open="props.open" @update:open="(v) => emit('update:open', v)">
    <AlertDialogContent>
      <AlertDialogHeader>
        <AlertDialogTitle>Delete upstream</AlertDialogTitle>
        <AlertDialogDescription>
          Delete <strong>{{ props.name }}</strong
          >? Its tools stop being proxied immediately. This cannot be undone.
        </AlertDialogDescription>
      </AlertDialogHeader>
      <AlertDialogFooter>
        <AlertDialogCancel>Cancel</AlertDialogCancel>
        <AlertDialogAction :disabled="props.loading" @click.prevent="emit('confirm')">
          Delete
        </AlertDialogAction>
      </AlertDialogFooter>
    </AlertDialogContent>
  </AlertDialog>
</template>
