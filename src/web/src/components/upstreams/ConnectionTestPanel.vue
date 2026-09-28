<script setup lang="ts">
import { ZapIcon } from '@lucide/vue'

import { Alert, AlertDescription } from '@/components/ui/alert'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table'
import { useTestUnsavedConnection } from '@/queries/useTestConnection'
import { failureKindLabels } from '@/types/labels'
import type { CreateUpstreamRequest } from '@/types/upstream'

const props = defineProps<{
  buildRequest: () => CreateUpstreamRequest
}>()

const testConnection = useTestUnsavedConnection()

function runTest() {
  testConnection.mutate(props.buildRequest())
}
</script>

<template>
  <div class="flex flex-col gap-3">
    <div class="flex items-center gap-3">
      <Button type="button" :disabled="testConnection.isPending.value" @click="runTest">
        <ZapIcon class="size-4" />
        Test & discover tools
      </Button>
      <Badge v-if="testConnection.data.value?.success" variant="default">
        Connected · {{ testConnection.data.value.tools.length }} tools ·
        {{ Math.round(testConnection.data.value.latencyMs) }}ms
      </Badge>
    </div>

    <Alert
      v-if="testConnection.data.value && !testConnection.data.value.success"
      variant="destructive"
    >
      <AlertDescription>
        {{ failureKindLabels[testConnection.data.value.failureKind] }}
        <template v-if="testConnection.data.value.message">
          — {{ testConnection.data.value.message }}</template
        >
      </AlertDescription>
    </Alert>
    <Alert v-else-if="testConnection.isError.value" variant="destructive">
      <AlertDescription>Could not run the connection test.</AlertDescription>
    </Alert>

    <Table v-if="testConnection.data.value?.success && testConnection.data.value.tools.length > 0">
      <TableHeader>
        <TableRow>
          <TableHead>Discovered tool</TableHead>
          <TableHead>Description</TableHead>
        </TableRow>
      </TableHeader>
      <TableBody>
        <TableRow v-for="tool in testConnection.data.value.tools" :key="tool.name">
          <TableCell class="font-mono">{{ tool.name }}</TableCell>
          <TableCell class="text-muted-foreground">{{ tool.description }}</TableCell>
        </TableRow>
      </TableBody>
    </Table>
  </div>
</template>
